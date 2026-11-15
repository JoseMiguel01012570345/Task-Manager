import { useEffect, useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { describeHttpError } from "../api/errors";
import { createTask, getTask, updateTask } from "../api/tasks";
import { DateTimePicker } from "../components/DateTimePicker";

export function TaskFormPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const editing = Boolean(id);
  const [title, setTitle] = useState("");
  const [description, setDescription] = useState("");
  const [assigneeId, setAssigneeId] = useState("dev-user");
  const [dueDate, setDueDate] = useState("");
  const [version, setVersion] = useState<number | undefined>();
  const [loading, setLoading] = useState(editing);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    if (!id) {
      return;
    }
    let active = true;
    getTask(id)
      .then((task) => {
        if (!active) {
          return;
        }
        setTitle(task.title);
        setDescription(task.description);
        setAssigneeId(task.assigneeId);
        setDueDate(toLocalInput(task.dueDate));
        setVersion(task.version);
      })
      .catch((caught: unknown) => {
        if (active) {
          setError(describeHttpError(caught));
        }
      })
      .finally(() => {
        if (active) {
          setLoading(false);
        }
      });
    return () => {
      active = false;
    };
  }, [id]);

  async function onSubmit(event: FormEvent) {
    event.preventDefault();
    setError(null);
    const input = {
      title,
      description,
      assigneeId,
      dueDate: dueDate ? new Date(dueDate).toISOString() : null,
      version,
    };
    try {
      const saved = editing && id ? await updateTask(id, input) : await createTask(input);
      void navigate(`/tasks/${saved.id}`);
    } catch (caught) {
      setError(describeHttpError(caught));
    }
  }

  if (loading) {
    return <p className="notice panel">Cargando…</p>;
  }

  return (
    <section className="page narrow">
      <header className="page-head">
        <div>
          <p className="eyebrow">{editing ? "Actualizar" : "Crear"}</p>
          <h1>{editing ? "Editar tarea" : "Nueva tarea"}</h1>
        </div>
      </header>
      {error && (
        <p className="error" role="alert">
          {error}
        </p>
      )}
      <form className="panel stack" onSubmit={(event) => void onSubmit(event)}>
        <label>
          <span>Título</span>
          <input value={title} onChange={(event) => setTitle(event.target.value)} required maxLength={200} />
        </label>
        <label>
          <span>Descripción</span>
          <textarea value={description} onChange={(event) => setDescription(event.target.value)} maxLength={4000} />
        </label>
        <label>
          <span>Responsable</span>
          <input value={assigneeId} onChange={(event) => setAssigneeId(event.target.value)} required maxLength={128} />
        </label>
        <label>
          <span>Fecha límite</span>
          <DateTimePicker value={dueDate} onChange={setDueDate} />
        </label>
        <div className="toolbar">
          <button type="submit">Guardar</button>
          <Link className="button ghost" to={id ? `/tasks/${id}` : "/"}>
            Cancelar
          </Link>
        </div>
      </form>
    </section>
  );
}

function toLocalInput(value: string | null): string {
  if (!value) {
    return "";
  }
  const date = new Date(value);
  const pad = (part: number) => String(part).padStart(2, "0");
  return `${date.getFullYear()}-${pad(date.getMonth() + 1)}-${pad(date.getDate())}T${pad(date.getHours())}:${pad(date.getMinutes())}`;
}
