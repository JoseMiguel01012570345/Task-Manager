import { useEffect } from "react";
import { Link, Route, Routes, useNavigate } from "react-router-dom";
import { setUnauthorizedHandler } from "./api/client";
import { clearSession, hasSession } from "./api/session";
import { LoginPage } from "./pages/LoginPage";
import { SessionPage } from "./pages/SessionPage";
import { TaskDetailPage } from "./pages/TaskDetailPage";
import { TaskFormPage } from "./pages/TaskFormPage";
import { TaskListPage } from "./pages/TaskListPage";

function SessionBridge() {
  const navigate = useNavigate();
  useEffect(() => {
    setUnauthorizedHandler(() => {
      clearSession();
      navigate("/login");
    });
    return () => setUnauthorizedHandler(null);
  }, [navigate]);
  return null;
}

function Header() {
  const navigate = useNavigate();
  return (
    <header>
      <Link to="/">Task Management</Link>
      <nav>
        <Link to="/tasks/new">Nueva</Link>
        {hasSession() ? (
          <button
            type="button"
            onClick={() => {
              clearSession();
              navigate("/login");
            }}
          >
            Salir
          </button>
        ) : (
          <Link to="/login">Entrar</Link>
        )}
      </nav>
    </header>
  );
}

export function App() {
  return (
    <>
      <SessionBridge />
      <Header />
      <main>
        <Routes>
          <Route path="/" element={<TaskListPage />} />
          <Route path="/login" element={<LoginPage />} />
          <Route path="/session" element={<SessionPage />} />
          <Route path="/tasks/new" element={<TaskFormPage />} />
          <Route path="/tasks/:id" element={<TaskDetailPage />} />
          <Route path="/tasks/:id/edit" element={<TaskFormPage />} />
        </Routes>
      </main>
    </>
  );
}
