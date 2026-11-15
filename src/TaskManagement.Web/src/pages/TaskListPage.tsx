import { useEffect, useState, type CSSProperties } from "react";
import { Link } from "react-router-dom";
import { describeHttpError } from "../api/errors";
import { listTasks } from "../api/tasks";
import type { TaskItem } from "../api/types";
import { StatusBadge } from "../components/StatusBadge";
import { formatDue, taskStatuses } from "../format";

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
    <section className="page">
      <header className="page-head">
        <div>
          <p className="eyebrow">Tablero</p>
          <h1>Tareas</h1>
        </div>
        <Link className="button" to="/tasks/new">
          Nueva tarea
        </Link>
      </header>
      <form
        className="panel filters"
        onSubmit={(event) => {
          event.preventDefault();
          setPage(1);
          setApplied({ status, assigneeId: assigneeId.trim() });
        }}
      >
        <label>
          <span>Estado</span>
          <select value={status} onChange={(event) => setStatus(event.target.value)}>
            <option value="">Todos</option>
            {taskStatuses.map((item) => (
              <option key={item.value} value={item.value}>
                {item.label}
              </option>
            ))}
          </select>
        </label>
        <label>
          <span>Responsable</span>
          <input
            value={assigneeId}
            placeholder="Identificador"
            onChange={(event) => setAssigneeId(event.target.value)}
          />
        </label>
        <button type="submit">Filtrar</button>
      </form>
      {error && (
        <p className="error" role="alert">
          {error}
        </p>
      )}
      {(loading || !error) && (
      <div className="panel table-wrap">
        {loading && <p className="notice">Cargando…</p>}
        {!loading && items.length === 0 && <p className="notice">No hay tareas para este filtro.</p>}
        {!loading && items.length > 0 && (
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
              {items.map((task, index) => (
                <tr key={task.id} style={{ "--row-delay": `${index * 45}ms` } as CSSProperties}>
                  <td>
                    <Link className="task-link" to={`/tasks/${task.id}`}>
                      {task.title}
                    </Link>
                  </td>
                  <td>
                    <StatusBadge status={task.status} />
                  </td>
                  <td>{task.assigneeId}</td>
                  <td>{formatDue(task.dueDate)}</td>
                </tr>
              ))}
            </tbody>
          </table>
        )}
      </div>
      )}
      {!error && (
      <div className="panel pager">
        <button type="button" className="ghost" disabled={page <= 1} onClick={() => setPage((current) => current - 1)}>
          Anterior
        </button>
        <span>
          Página {page} de {lastPage} · {total} {total === 1 ? "tarea" : "tareas"}
        </span>
        <button
          type="button"
          className="ghost"
          disabled={page >= lastPage}
          onClick={() => setPage((current) => current + 1)}
        >
          Siguiente
        </button>
      </div>
      )}
    </section>
  );
}
