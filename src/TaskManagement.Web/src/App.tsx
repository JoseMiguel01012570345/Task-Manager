import { useEffect } from "react";
import { Link, Route, Routes, useNavigate } from "react-router-dom";
import { setUnauthorizedHandler } from "./api/client";
import { SessionPage } from "./pages/SessionPage";
import { TaskDetailPage } from "./pages/TaskDetailPage";
import { TaskFormPage } from "./pages/TaskFormPage";
import { TaskListPage } from "./pages/TaskListPage";

function SessionBridge() {
  const navigate = useNavigate();
  useEffect(() => {
    setUnauthorizedHandler(() => navigate("/session"));
    return () => setUnauthorizedHandler(null);
  }, [navigate]);
  return null;
}

export function App() {
  return (
    <>
      <SessionBridge />
      <header>
        <Link to="/">Task Management</Link>
        <nav>
          <Link to="/tasks/new">Nueva</Link>
        </nav>
      </header>
      <main>
        <Routes>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/session" element={<SessionPage />} />
          <Route path="/tasks/new" element={<TaskFormPage />} />
          <Route path="/tasks/:id" element={<TaskDetailPage />} />
          <Route path="/tasks/:id/edit" element={<TaskFormPage />} />
        </Routes>
      </main>
    </>
  );
}
