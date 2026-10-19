export default function UnauthorizedPage() {
  return (
    <main className="flex min-h-screen flex-col items-center justify-center gap-4 p-8 text-center">
      <h1 className="text-2xl font-semibold">ไม่มีสิทธิ์เข้าใช้งาน</h1>
      <p className="text-muted-foreground">
        อีเมลของคุณไม่ได้รับอนุญาตให้เข้าใช้งานระบบ
        <br />
        กรุณาติดต่อผู้ดูแลระบบ
      </p>
    </main>
  );
}
