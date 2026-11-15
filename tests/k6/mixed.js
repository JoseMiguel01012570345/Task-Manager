import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8088";
const suite = __ENV.SUITE === "1";

export const options = {
  vus: suite ? 3 : 8,
  duration: suite ? "8s" : "2m",
  gracefulStop: "1s",
};

export default function () {
  const headers = __ENV.TOKEN ? { Authorization: `Bearer ${__ENV.TOKEN}` } : {};
  if (Math.random() < 0.7) {
    const response = http.get(`${base}/api/v1/tasks`, { headers });
    check(response, { "list answered": (result) => result.status === 200 || result.status === 401 || result.status === 429 });
  } else {
    const response = http.post(
      `${base}/api/v1/tasks`,
      JSON.stringify({ title: "Carga mixta", assigneeId: "dev-user" }),
      { headers: { ...headers, "Content-Type": "application/json" } },
    );
    check(response, {
      "write answered": (result) => [201, 400, 401, 403, 429].includes(result.status),
    });
  }
  sleep(0.3);
}
