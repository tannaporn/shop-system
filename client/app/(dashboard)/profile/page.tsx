import { getUser } from "@/features/auth/api/auth.server";

export default async function ProfilePage() {
  const user = await getUser();

  return (
    <div className="space-y-4">
      <h1 className="text-2xl font-bold">Profile</h1>
      <div className="bg-card rounded-xl p-6 shadow-sm space-y-3">
        <div>
          <p className="text-xs text-muted">Email</p>
          <p className="text-sm font-medium">{user?.email}</p>
        </div>
        <div>
          <p className="text-xs text-muted">User ID</p>
          <p className="text-sm font-medium">{user?.id}</p>
        </div>
      </div>
    </div>
  );
}
