"use client";

import { useState } from "react";
import { apiClient } from "@/lib/api/client";

type AddToCartButtonProps = {
  productId: string;
  disabled?: boolean;
};

export function AddToCartButton({ productId, disabled }: AddToCartButtonProps) {
  const [loading, setLoading] = useState(false);
  const [added, setAdded] = useState(false);
  const [error, setError] = useState<string | null>(null);

  async function handleClick() {
    setLoading(true);
    setError(null);
    try {
      await apiClient.post("/api/cart/items", { productId, quantity: 1 });
      setAdded(true);
      setTimeout(() => setAdded(false), 2000);
    } catch (e) {
      setError(e instanceof Error ? e.message : "เกิดข้อผิดพลาด");
    } finally {
      setLoading(false);
    }
  }

  return (
    <div className="flex flex-col items-end gap-1">
      <button
        onClick={handleClick}
        disabled={disabled || loading}
        className="px-3 py-1.5 text-xs rounded-md bg-primary text-primary-foreground hover:bg-primary-hover font-semibold disabled:opacity-50 disabled:cursor-not-allowed"
      >
        {loading ? "กำลังเพิ่ม..." : added ? "เพิ่มแล้ว ✓" : "Add to Cart"}
      </button>
      {error && <p className="text-xs text-red-500">{error}</p>}
    </div>
  );
}
