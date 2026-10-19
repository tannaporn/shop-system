export function Spinner({ className = "" }: { className?: string }) {
  return (
    <div
      role="status"
      aria-label="กำลังโหลด"
      className={`h-6 w-6 animate-spin rounded-full border-2 border-primary border-t-transparent ${className}`}
    />
  );
}
