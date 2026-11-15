import type { TaskStatus } from "./api/types";

export const taskStatuses: { value: TaskStatus; label: string }[] = [
  { value: "Pending", label: "Pendiente" },
  { value: "InProgress", label: "En curso" },
  { value: "Done", label: "Hecha" },
];

export function statusLabel(status: TaskStatus): string {
  return taskStatuses.find((item) => item.value === status)?.label ?? status;
}

export function formatDue(value: string | null): string {
  if (!value) {
    return "Sin fecha";
  }
  return new Date(value).toLocaleString("es", { dateStyle: "medium", timeStyle: "short" });
}
