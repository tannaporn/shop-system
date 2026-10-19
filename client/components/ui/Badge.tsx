import type { OrderStatus } from "@/types/order";

type BadgeVariant = OrderStatus | "out-of-stock";

const classes: Record<BadgeVariant, string> = {
  Pending: "bg-yellow-100 text-yellow-800",
  Confirmed: "bg-blue-100 text-blue-800",
  Completed: "bg-green-100 text-green-800",
  Cancelled: "bg-red-100 text-red-800",
  "out-of-stock": "bg-gray-100 text-gray-600",
};

const labels: Record<BadgeVariant, string> = {
  Pending: "Pending",
  Confirmed: "Confirmed",
  Completed: "Completed",
  Cancelled: "Cancelled",
  "out-of-stock": "Out of Stock",
};

type BadgeProps = {
  variant: BadgeVariant;
};

export function Badge({ variant }: BadgeProps) {
  return (
    <span
      className={`inline-flex items-center px-2.5 py-0.5 rounded-full text-xs font-medium ${classes[variant]}`}
    >
      {labels[variant]}
    </span>
  );
}
