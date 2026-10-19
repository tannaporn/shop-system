import { redirect } from "next/navigation";
import { getSession } from "@/features/auth/api/auth.server";

export default async function AdminLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getSession();
  if (!session) redirect("/login");

  return (
    <div className="min-h-screen bg-background">
      <header className="border-b border-border px-8 py-4">
        <h1 className="text-lg font-semibold">Admin</h1>
      </header>
      <main className="p-8">{children}</main>
    </div>
  );
}
