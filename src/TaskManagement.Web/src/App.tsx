import { Link, Route, Routes } from "react-router-dom";
import { TaskDetailPage } from "./pages/TaskDetailPage";
import { TaskFormPage } from "./pages/TaskFormPage";
import { TaskListPage } from "./pages/TaskListPage";

export function App() {
  return (
    <>
      <header>
        <Link to="/">Task Management</Link>
        <nav>
          <Link to="/tasks/new">Nueva</Link>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/tasks/new" element={<TaskFormPage />} />
          <Route path="/tasks/:id" element={<TaskDetailPage />} />
          <Route path="/tasks/:id/edit" element={<TaskFormPage />} />
        </Routes>
      </main>
    </>
  );
}
