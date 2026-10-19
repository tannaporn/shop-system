import { redirect } from "next/navigation";
import { cookies } from "next/headers";
import { getSession } from "@/features/auth/api/auth.server";
import { Sidebar } from "@/components/layout/Sidebar";

export default async function DashboardLayout({
  children,
}: {
  children: React.ReactNode;
}) {
  const session = await getSession();
  if (!session) redirect("/login");

  const cookieStore = await cookies();
  const role = cookieStore.get("x-user-role")?.value;

  return (
    <div className="flex h-screen bg-background">
      <Sidebar role={role} />
      <main className="flex-1 overflow-y-auto p-8">{children}</main>
    </div>
  );
}
