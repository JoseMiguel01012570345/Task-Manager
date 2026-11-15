import http from "k6/http";
import { check, sleep } from "k6";

const base = __ENV.BASE_URL || "http://localhost:8090";

export const options = {
  vus: 5,
  duration: "10m",
  thresholds: {
    http_req_failed: ["rate<0.05"],
  },
};

export default function () {
  const response = http.get(`${base}/health/live`);
  check(response, { "live responds 200": (result) => result.status === 200 });
  sleep(1);
}
