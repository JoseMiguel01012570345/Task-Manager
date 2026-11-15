import { useEffect, useState } from "react";
import { Link } from "react-router-dom";
import { listTasks } from "../api/tasks";
import type { TaskItem, TaskStatus } from "../api/types";
import { describeHttpError } from "../api/errors";

const pageSize = 10;

export function TaskListPage() {
  const [items, setItems] = useState<TaskItem[]>([]);
  const [page, setPage] = useState(1);
  const [total, setTotal] = useState(0);
  const [status, setStatus] = useState("");
  const [assigneeId, setAssigneeId] = useState("");
  const [applied, setApplied] = useState({ status: "", assigneeId: "" });
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  useEffect(() => {
    let active = true;
    setLoading(true);
    setError(null);
    listTasks({
      page,
      pageSize,
      status: applied.status || undefined,
      assigneeId: applied.assigneeId || undefined,
    })
      .then((result) => {
        if (!active) {
          return;
        }
        setItems(result.items);
        setTotal(result.totalCount);
      })
      .catch((caught: unknown) => {
        if (!active) {
          return;
        }
        setError(describeHttpError(caught));
      })
      .finally(() => {
        if (active) {
          setLoading(false);
        }
      });
    return () => {
      active = false;
    };
  }, [page, applied]);

  const lastPage = Math.max(1, Math.ceil(total / pageSize));

  return (
    <section>
      <div className="row">
        <h1>Tareas</h1>
        <Link to="/tasks/new">Nueva tarea</Link>
      </div>
      <form
        className="filters"
        onSubmit={(event) => {
          event.preventDefault();
          setPage(1);
          setApplied({ status, assigneeId: assigneeId.trim() });
        }}
      >
        <label>
          Estado
          <select value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">Todos</option>
            <option value="Pending">Pending</option>
            <option value="InProgress">InProgress</option>
            <option value="Done">Done</option>
          </select>
        </label>
        <label>
          Responsable
          <input value={assigneeId} onChange={(event) => setAssigneeId(event.target.value)} />
        </label>
        <button type="submit">Filtrar</button>
      </form>
      {loading && <p>Cargando...</p>}
      {error && <p className="error">{error}</p>}
      {!loading && !error && items.length === 0 && <p>No hay tareas para este filtro.</p>}
      {!loading && !error && items.length > 0 && (
        <table>
          <thead>
            <tr>
              <th>Título</th>
              <th>Estado</th>
              <th>Responsable</th>
              <th>Límite</th>
            </tr>
          </thead>
          <tbody>
            {items.map((task) => (
              <tr key={task.id}>
                <td>
                  <Link to={`/tasks/${task.id}`}>{task.title}</Link>
                </td>
                <td>{task.status as TaskStatus}</td>
                <td>{task.assigneeId}</td>
                <td>{task.dueDate ? new Date(task.dueDate).toLocaleString() : "—"}</td>
              </tr>
            ))}
          </tbody>
        </table>
      )}
      <div className="row">
        <button type="button" disabled={page <= 1} onClick={() => setPage((current) => current - 1)}>
          Anterior
        </button>
        <span>
          Página {page} de {lastPage} ({total})
        </span>
        <button type="button" disabled={page >= lastPage} onClick={() => setPage((current) => current + 1)}>
          Siguiente
        </button>
      </div>
    </section>
  );
}
