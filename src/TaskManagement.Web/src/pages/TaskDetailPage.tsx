import { useEffect, useState } from "react";
import { Link, useNavigate, useParams } from "react-router-dom";
import { describeHttpError } from "../api/errors";
import { changeTaskStatus, deleteTask, getTask } from "../api/tasks";
import type { TaskItem, TaskStatus } from "../api/types";
import { StatusBadge } from "../components/StatusBadge";
import { formatDue, statusLabel, taskStatuses } from "../format";

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

  async function onStatus() {
    if (!task) {
      return;
    }
    setError(null);
    try {
      setTask(await changeTaskStatus(task.id, nextStatus, task.version));
    } catch (caught) {
      setError(describeHttpError(caught));
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
      setError(describeHttpError(caught));
    }
  }

  if (loading) {
    return <p className="notice panel">Cargando…</p>;
  }
  if (!task) {
    return (
      <p className="error" role="alert">
        {error ?? "Tarea no encontrada."}
      </p>
    );
  }

  return (
    <section className="page">
      <Link className="back-link" to="/">
        Volver al listado
      </Link>
      <header className="page-head">
        <div>
          <StatusBadge status={task.status} />
          <h1>{task.title}</h1>
        </div>
      </header>
      {error && (
        <p className="error" role="alert">
          {error}
        </p>
      )}
      <article className="panel detail-body">
        <p className="description">{task.description || "Sin descripción."}</p>
        <dl className="meta">
          <div>
            <dt>Responsable</dt>
            <dd>{task.assigneeId}</dd>
          </div>
          <div>
            <dt>Creada por</dt>
            <dd>{task.createdById}</dd>
          </div>
          <div>
            <dt>Fecha límite</dt>
            <dd>{formatDue(task.dueDate)}</dd>
          </div>
          <div>
            <dt>Versión</dt>
            <dd>{task.version}</dd>
          </div>
        </dl>
      </article>
      <div className="panel toolbar">
        <label>
          <span>Nuevo estado</span>
          <select value={nextStatus} onChange={(event) => setNextStatus(event.target.value as TaskStatus)}>
            {taskStatuses.map((status) => (
              <option key={status.value} value={status.value}>
                {status.label}
              </option>
            ))}
          </select>
        </label>
        <button type="button" onClick={() => void onStatus()}>
          Cambiar a {statusLabel(nextStatus)}
        </button>
        <Link className="button ghost" to={`/tasks/${task.id}/edit`}>
          Editar
        </Link>
        <button type="button" className={confirmDelete ? "danger solid" : "danger"} onClick={() => void onDelete()}>
          {confirmDelete ? "Confirmar eliminación" : "Eliminar"}
        </button>
      </div>
    </section>
  );
}
