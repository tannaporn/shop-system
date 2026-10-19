"use client";

import { useState, useTransition } from "react";
import { useRouter } from "next/navigation";
import { updateOrderStatus } from "@/features/admin/orders/actions";
import type { OrderStatus } from "@/types/order";

type Props = {
  orderId: string;
  status: OrderStatus;
};

export function OrderActions({ orderId, status }: Props) {
  const router = useRouter();
  const [isPending, startTransition] = useTransition();
  const [error, setError] = useState<string | null>(null);

  function handleUpdate(newStatus: OrderStatus) {
    setError(null);
    startTransition(async () => {
      try {
        await updateOrderStatus(orderId, newStatus);
        router.refresh();
      } catch (e: unknown) {
        setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
      }
    });
  }

  return (
    <div className="flex flex-col items-end gap-1">
      <div className="inline-flex gap-2">
        {status === "Pending" && (
          <button
            disabled={isPending}
            onClick={() => handleUpdate("Confirmed")}
            className="rounded px-2 py-1 text-xs font-medium text-blue-700 border border-blue-200 hover:bg-blue-50 transition-colors disabled:opacity-50"
          >
            Confirm
          </button>
        )}
        {status === "Confirmed" && (
          <button
            disabled={isPending}
            onClick={() => handleUpdate("Completed")}
            className="rounded px-2 py-1 text-xs font-medium text-green-700 border border-green-200 hover:bg-green-50 transition-colors disabled:opacity-50"
          >
            Complete
          </button>
        )}
        {(status === "Pending" || status === "Confirmed") && (
          <button
            disabled={isPending}
            onClick={() => handleUpdate("Cancelled")}
            className="rounded px-2 py-1 text-xs font-medium text-red-600 border border-red-200 hover:bg-red-50 transition-colors disabled:opacity-50"
          >
            Cancel
          </button>
        )}
      </div>
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
}
