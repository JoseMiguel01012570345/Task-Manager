import { useEffect, useState, type FormEvent } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api/client";
import { createTask, getTask, updateTask } from "../api/tasks";

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
          setError(caught instanceof ApiError ? `Error ${caught.status}` : "No se pudo cargar la tarea.");
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
      setError(caught instanceof ApiError ? `Error ${caught.status}` : "No se pudo guardar la tarea.");
    }
  }

  if (loading) {
    return <p>Cargando...</p>;
  }

  return (
    <section>
      <h1>{editing ? "Editar tarea" : "Nueva tarea"}</h1>
      {error && <p className="error">{error}</p>}
      <form className="stack" onSubmit={(event) => void onSubmit(event)}>
        <label>
          Título
          <input value={title} onChange={(event) => setTitle(event.target.value)} required maxLength={200} />
        </label>
        <label>
          Descripción
          <textarea value={description} onChange={(event) => setDescription(event.target.value)} maxLength={4000} />
        </label>
        <label>
          Responsable
          <input value={assigneeId} onChange={(event) => setAssigneeId(event.target.value)} required maxLength={128} />
        </label>
        <label>
          Fecha límite
          <input type="datetime-local" value={dueDate} onChange={(event) => setDueDate(event.target.value)} />
        </label>
        <div className="row">
          <button type="submit">Guardar</button>
          <Link to={id ? `/tasks/${id}` : "/"}>Cancelar</Link>
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
