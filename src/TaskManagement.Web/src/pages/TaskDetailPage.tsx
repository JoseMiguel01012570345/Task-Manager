import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { ApiError } from "../api/client";
import { changeTaskStatus, deleteTask, getTask } from "../api/tasks";
import type { TaskItem, TaskStatus } from "../api/types";

const statuses: TaskStatus[] = ["Pending", "InProgress", "Done"];

export function TaskDetailPage() {
  const { id } = useParams();
  const navigate = useNavigate();
  const [task, setTask] = useState<TaskItem | null>(null);
  const [nextStatus, setNextStatus] = useState<TaskStatus>("InProgress");
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);
  const [confirmDelete, setConfirmDelete] = useState(false);

  useEffect(() => {
    if (!id) {
      return;
    }
    let active = true;
    getTask(id)
      .then((loaded) => {
        if (!active) {
          return;
        }
        setTask(loaded);
        setNextStatus(loaded.status === "Pending" ? "InProgress" : "Done");
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

  async function onStatus() {
    if (!task) {
      return;
    }
    setError(null);
    try {
      setTask(await changeTaskStatus(task.id, nextStatus, task.version));
    } catch (caught) {
      setError(caught instanceof ApiError ? `Error ${caught.status}` : "No se pudo cambiar el estado.");
    }
  }

  async function onDelete() {
    if (!task) {
      return;
    }
    if (!confirmDelete) {
      setConfirmDelete(true);
      return;
    }
    try {
      await deleteTask(task.id, task.version);
      void navigate("/");
    } catch (caught) {
      setConfirmDelete(false);
      setError(caught instanceof ApiError ? `Error ${caught.status}` : "No se pudo eliminar la tarea.");
    }
  }

  if (loading) {
    return <p>Cargando...</p>;
  }
  if (!task) {
    return <p className="error">{error ?? "Tarea no encontrada."}</p>;
  }

  return (
    <section>
      <p>
        <Link to="/">Volver al listado</Link>
      </p>
      <h1>{task.title}</h1>
      {error && <p className="error">{error}</p>}
      <p>{task.description || "Sin descripción."}</p>
      <dl>
        <dt>Estado</dt>
        <dd>{task.status}</dd>
        <dt>Responsable</dt>
        <dd>{task.assigneeId}</dd>
        <dt>Creada por</dt>
        <dd>{task.createdById}</dd>
        <dt>Fecha límite</dt>
        <dd>{task.dueDate ? new Date(task.dueDate).toLocaleString() : "—"}</dd>
        <dt>Versión</dt>
        <dd>{task.version}</dd>
      </dl>
      <div className="row">
        <select value={nextStatus} onChange={(event) => setNextStatus(event.target.value as TaskStatus)}>
          {statuses.map((status) => (
            <option key={status} value={status}>
              {status}
            </option>
          ))}
        </select>
        <button type="button" onClick={() => void onStatus()}>
          Cambiar estado
        </button>
        <Link to={`/tasks/${task.id}/edit`}>Editar</Link>
        <button type="button" onClick={() => void onDelete()}>
          {confirmDelete ? "Confirmar eliminación" : "Eliminar"}
        </button>
      </div>
    </section>
  );
}
