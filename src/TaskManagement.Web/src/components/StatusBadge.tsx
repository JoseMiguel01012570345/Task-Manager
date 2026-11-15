import type { TaskStatus } from "../api/types";
import { statusLabel } from "../format";

export function StatusBadge({ status }: { status: TaskStatus }) {
  return <span className={`badge badge-${status}`}>{statusLabel(status)}</span>;
}
