import { useEffect, useId, useRef, useState, type KeyboardEvent as ReactKeyboardEvent } from "react";

const weekdays = ["L", "M", "X", "J", "V", "S", "D"];
const months = [
  "enero",
  "febrero",
  "marzo",
  "abril",
  "mayo",
  "junio",
  "julio",
  "agosto",
  "septiembre",
  "octubre",
  "noviembre",
  "diciembre",
];

interface DateTimePickerProps {
  value: string;
  onChange: (value: string) => void;
}

export function DateTimePicker({ value, onChange }: DateTimePickerProps) {
  const labelId = useId();
  const rootRef = useRef<HTMLDivElement>(null);
  const selected = parseLocal(value);
  const [open, setOpen] = useState(false);
  const [above, setAbove] = useState(false);
  const [hour, setHour] = useState(selected?.getHours() ?? 9);
  const [minute, setMinute] = useState(selected?.getMinutes() ?? 0);
  const [view, setView] = useState(() => startOfMonth(selected ?? new Date()));
  const [motion, setMotion] = useState<"next" | "prev" | null>(null);

  useEffect(() => {
    if (!open) {
      return;
    }
    function onPointer(event: PointerEvent) {
      if (!rootRef.current?.contains(event.target as Node)) {
        setOpen(false);
      }
    }
    function onKey(event: KeyboardEvent) {
      if (event.key === "Escape") {
        setOpen(false);
      }
    }
    document.addEventListener("pointerdown", onPointer);
    document.addEventListener("keydown", onKey);
    return () => {
      document.removeEventListener("pointerdown", onPointer);
      document.removeEventListener("keydown", onKey);
    };
  }, [open]);

  function toggle() {
    if (open) {
      setOpen(false);
      return;
    }
    const rect = rootRef.current?.getBoundingClientRect();
    if (rect) {
      const spaceBelow = window.innerHeight - rect.bottom;
      const spaceAbove = rect.top;
      setAbove(spaceBelow < 470 && spaceAbove > spaceBelow);
    }
    const current = parseLocal(value);
    setHour(current?.getHours() ?? 9);
    setMinute(current?.getMinutes() ?? 0);
    setView(startOfMonth(current ?? new Date()));
    setMotion(null);
    setOpen(true);
  }

  useEffect(() => {
    if (!open) {
      return;
    }
    rootRef.current?.querySelector<HTMLButtonElement>(".picker-day.is-selected, .picker-day.is-today")?.focus();
  }, [open]);

  function shiftMonth(delta: number) {
    setMotion(delta > 0 ? "next" : "prev");
    setView((current) => new Date(current.getFullYear(), current.getMonth() + delta, 1));
  }

  function chooseDay(day: Date) {
    onChange(toLocalValue(new Date(day.getFullYear(), day.getMonth(), day.getDate(), hour, minute)));
    setView(startOfMonth(day));
  }

  function chooseTime(nextHour: number, nextMinute: number) {
    setHour(nextHour);
    setMinute(nextMinute);
    const base = selected ?? new Date();
    onChange(toLocalValue(new Date(base.getFullYear(), base.getMonth(), base.getDate(), nextHour, nextMinute)));
  }

  function onGridKeyDown(event: ReactKeyboardEvent<HTMLDivElement>) {
    const target = event.target as HTMLButtonElement;
    if (!target.dataset.date) {
      return;
    }
    const step = keyStep(event.key);
    if (step === 0) {
      return;
    }
    event.preventDefault();
    const [year, month, day] = target.dataset.date.split("-").map(Number);
    const next = new Date(year, month - 1, day + step);
    setMotion(step > 0 ? "next" : "prev");
    setView(startOfMonth(next));
    window.setTimeout(() => {
      rootRef.current?.querySelector<HTMLButtonElement>(`[data-date="${toDateKey(next)}"]`)?.focus();
    }, 0);
  }

  const days = buildDays(view);
  const minuteOptions = minuteOptionsFor(minute);

  return (
    <div className="picker" ref={rootRef}>
      <button
        type="button"
        className={value ? "picker-trigger" : "picker-trigger is-empty"}
        aria-haspopup="dialog"
        aria-expanded={open}
        onClick={toggle}
      >
        <span>{value && selected ? formatTrigger(selected) : "Elegir fecha y hora"}</span>
        <span className="picker-icon" aria-hidden="true" />
      </button>
      {open && (
        <div className={above ? "picker-popover above" : "picker-popover"} role="dialog" aria-labelledby={labelId}>
          <div className="picker-head">
            <button type="button" className="picker-nav" aria-label="Mes anterior" onClick={() => shiftMonth(-1)}>
              ‹
            </button>
            <p id={labelId} className="picker-month">
              {months[view.getMonth()]} {view.getFullYear()}
            </p>
            <button type="button" className="picker-nav" aria-label="Mes siguiente" onClick={() => shiftMonth(1)}>
              ›
            </button>
          </div>
          <div className="picker-weekdays" aria-hidden="true">
            {weekdays.map((day) => (
              <span key={day}>{day}</span>
            ))}
          </div>
          <div className="picker-window" onKeyDown={onGridKeyDown}>
            <div key={`${view.getFullYear()}-${view.getMonth()}`} className={motion ? `picker-grid slide-${motion}` : "picker-grid"}>
              {days.map(({ date, inMonth }) => {
                const key = toDateKey(date);
                const isSelected = selected ? sameDay(date, selected) : false;
                const isToday = sameDay(date, new Date());
                return (
                  <button
                    key={key}
                    type="button"
                    className={["picker-day", inMonth ? "" : "is-outside", isSelected ? "is-selected" : "", isToday ? "is-today" : ""].filter(Boolean).join(" ")}
                    data-date={key}
                    aria-pressed={isSelected}
                    onClick={() => chooseDay(date)}
                  >
                    {date.getDate()}
                  </button>
                );
              })}
            </div>
          </div>
          <div className="picker-time">
            <span>Hora</span>
            <select aria-label="Hora" value={hour} onChange={(event) => chooseTime(Number(event.target.value), minute)}>
              {Array.from({ length: 24 }, (_, index) => (
                <option key={index} value={index}>
                  {String(index).padStart(2, "0")}
                </option>
              ))}
            </select>
            <select aria-label="Minutos" value={minute} onChange={(event) => chooseTime(hour, Number(event.target.value))}>
              {minuteOptions.map((option) => (
                <option key={option} value={option}>
                  {String(option).padStart(2, "0")}
                </option>
              ))}
            </select>
          </div>
          <div className="picker-actions">
            <button
              type="button"
              onClick={() => {
                const now = new Date();
                setHour(now.getHours());
                setMinute(now.getMinutes());
                setView(startOfMonth(now));
                onChange(toLocalValue(now));
              }}
            >
              Hoy
            </button>
            <button
              type="button"
              className="ghost"
              onClick={() => {
                onChange("");
                setOpen(false);
              }}
            >
              Quitar
            </button>
          </div>
        </div>
      )}
    </div>
  );
}

function keyStep(key: string): number {
  switch (key) {
    case "ArrowLeft":
      return -1;
    case "ArrowRight":
      return 1;
    case "ArrowUp":
      return -7;
    case "ArrowDown":
      return 7;
    default:
      return 0;
  }
}

function buildDays(view: Date): { date: Date; inMonth: boolean }[] {
  const first = new Date(view.getFullYear(), view.getMonth(), 1);
  const startOffset = (first.getDay() + 6) % 7;
  const start = new Date(view.getFullYear(), view.getMonth(), 1 - startOffset);
  return Array.from({ length: 42 }, (_, index) => {
    const date = new Date(start.getFullYear(), start.getMonth(), start.getDate() + index);
    return { date, inMonth: date.getMonth() === view.getMonth() };
  });
}

function minuteOptionsFor(selected: number): number[] {
  const options = Array.from({ length: 12 }, (_, index) => index * 5);
  if (!options.includes(selected)) {
    options.push(selected);
    options.sort((left, right) => left - right);
  }
  return options;
}

function startOfMonth(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

function sameDay(left: Date, right: Date): boolean {
  return left.getFullYear() === right.getFullYear() && left.getMonth() === right.getMonth() && left.getDate() === right.getDate();
}

function parseLocal(value: string): Date | null {
  const match = /^(\d{4})-(\d{2})-(\d{2})T(\d{2}):(\d{2})/.exec(value);
  if (!match) {
    return null;
  }
  const [, year, month, day, hour, minute] = match;
  return new Date(Number(year), Number(month) - 1, Number(day), Number(hour), Number(minute));
}

function toLocalValue(date: Date): string {
  const pad = (part: number) => String(part).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}

function toDateKey(date: Date): string {
  const pad = (part: number) => String(part).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}`;
}

function formatTrigger(date: Date): string {
  return date.toLocaleString("es", { dateStyle: "medium", timeStyle: "short" });
}
