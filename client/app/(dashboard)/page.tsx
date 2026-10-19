import { getUser } from "@/features/auth/api/auth.server";

export default async function DashboardPage() {
  const user = await getUser();

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Dashboard</h1>
      <p className="text-muted">Welcome back, {user?.email}</p>
    </div>
  );
}
