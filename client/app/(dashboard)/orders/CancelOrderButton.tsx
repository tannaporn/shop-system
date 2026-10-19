"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import { apiClient } from "@/lib/api/client";
import { Modal } from "@/components/ui/Modal";

type CancelOrderButtonProps = {
  orderId: string;
};

export function CancelOrderButton({ orderId }: CancelOrderButtonProps) {
  const router = useRouter();
  const [open, setOpen] = useState(false);
  const [loading, setLoading] = useState(false);

  async function handleConfirm() {
    setLoading(true);
    try {
      await apiClient.post(`/api/orders/${orderId}/cancel`, {});
      setOpen(false);
      router.refresh();
    } catch {
      // keep modal open so user sees it failed
    } finally {
      setLoading(false);
    }
  }

  return (
    <>
      <button
        onClick={() => setOpen(true)}
        className="text-xs text-red-500 hover:text-red-700"
      >
        ยกเลิก
      </button>
      <Modal
        open={open}
        title="ยกเลิกคำสั่งซื้อ"
        description="คุณต้องการยกเลิกคำสั่งซื้อนี้ใช่หรือไม่? การดำเนินการนี้ไม่สามารถย้อนกลับได้"
        confirmLabel="ยืนยันการยกเลิก"
        onConfirm={handleConfirm}
        onCancel={() => setOpen(false)}
        loading={loading}
      />
    </>
  );
}
