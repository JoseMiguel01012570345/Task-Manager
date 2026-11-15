export type TaskStatus = "Pending" | "InProgress" | "Done";

export interface TaskItem {
  id: string;
  title: string;
  description: string;
  status: TaskStatus;
  assigneeId: string;
  createdById: string;
  dueDate: string | null;
  createdAt: string;
  updatedAt: string;
  version: number;
}

export interface TaskPage {
  items: TaskItem[];
  page: number;
  pageSize: number;
  totalCount: number;
}

export interface TaskInput {
  title: string;
  description: string;
  assigneeId: string;
  dueDate: string | null;
  version?: number;
}
