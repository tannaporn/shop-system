import { redirect } from "next/navigation";
import { getSession } from "@/features/auth/api/auth.server";
import { LoginForm } from "@/features/auth/components/LoginForm";

export const dynamic = "force-dynamic";

export default async function LoginPage() {
  // ถ้า login อยู่แล้ว ไม่ต้องมาหน้านี้
  const session = await getSession();
  if (session) redirect("/");

  return (
    <main className="flex min-h-screen items-center justify-center bg-background px-4">
      <LoginForm />
    </main>
  );
}
