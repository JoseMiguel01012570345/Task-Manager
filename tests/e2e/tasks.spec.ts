import { expect, test, type APIRequestContext } from "@playwright/test";

const base = process.env.BASE_URL || "http://localhost:8088";
const password = process.env.DEV_USER_PASSWORD || "lab-dev-user";

test("create and read a task", async ({ request }) => {
  const accessToken = await token(request);
  const created = await request.post(`${base}/api/v1/tasks`, {
    headers: { Authorization: `Bearer ${accessToken}` },
    data: { title: "Flujo e2e", description: "Playwright", assigneeId: "dev-user" },
  });
  expect(created.status()).toBe(201);
  const body = (await created.json()) as { id: string; title: string; version: number };

  const fetched = await request.get(`${base}/api/v1/tasks/${body.id}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  expect(fetched.status()).toBe(200);
  expect(((await fetched.json()) as { title: string }).title).toBe("Flujo e2e");

  const removed = await request.delete(`${base}/api/v1/tasks/${body.id}?version=${body.version}`, {
    headers: { Authorization: `Bearer ${accessToken}` },
  });
  expect(removed.status()).toBe(204);
});

async function token(request: APIRequestContext) {
  const response = await request.post(`${base}/realms/taskmanagement/protocol/openid-connect/token`, {
    form: {
      grant_type: "password",
      client_id: "taskmanagement-web",
      username: "dev-user",
      password,
    },
  });
  expect(response.ok()).toBeTruthy();
  const body = (await response.json()) as { access_token: string };
  return body.access_token;
}
