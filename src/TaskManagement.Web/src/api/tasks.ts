import { api } from "./client";
import type { TaskInput, TaskItem, TaskPage, TaskStatus } from "./types";

export interface TaskQuery {
  status?: string;
  assigneeId?: string;
  page: number;
  pageSize: number;
}

export function listTasks(query: TaskQuery): Promise<TaskPage> {
  const params = new URLSearchParams({
    page: String(query.page),
    pageSize: String(query.pageSize),
  });
  if (query.status) {
    params.set("status", query.status);
  }
  if (query.assigneeId) {
    params.set("assigneeId", query.assigneeId);
  }
  return api<TaskPage>(`/api/v1/tasks?${params.toString()}`);
}

export function getTask(id: string): Promise<TaskItem> {
  return api<TaskItem>(`/api/v1/tasks/${id}`);
}

export function createTask(input: TaskInput): Promise<TaskItem> {
  return api<TaskItem>("/api/v1/tasks", {
    method: "POST",
    body: JSON.stringify({
      title: input.title,
      description: input.description,
      assigneeId: input.assigneeId,
      dueDate: input.dueDate,
    }),
  });
}

export function updateTask(id: string, input: TaskInput): Promise<TaskItem> {
  return api<TaskItem>(`/api/v1/tasks/${id}`, {
    method: "PUT",
    body: JSON.stringify({
      title: input.title,
      description: input.description,
      assigneeId: input.assigneeId,
      dueDate: input.dueDate,
      version: input.version,
    }),
  });
}

export function changeTaskStatus(id: string, status: TaskStatus, version: number): Promise<TaskItem> {
  return api<TaskItem>(`/api/v1/tasks/${id}/status`, {
    method: "PATCH",
    body: JSON.stringify({ status, version }),
  });
}

export function deleteTask(id: string, version: number): Promise<void> {
  return api<void>(`/api/v1/tasks/${id}?version=${version}`, { method: "DELETE" });
}
