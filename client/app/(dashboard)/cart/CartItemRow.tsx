"use client";

import { useState } from "react";
import { useRouter } from "next/navigation";
import type { CartItem } from "@/types/cart";
import { apiClient } from "@/lib/api/client";

type CartItemRowProps = {
  item: CartItem;
};

export function CartItemRow({ item }: CartItemRowProps) {
  const router = useRouter();
  const [quantity, setQuantity] = useState(item.quantity);
  const [loading, setLoading] = useState(false);

  async function updateQuantity(newQty: number) {
    if (newQty < 1) return;
    setLoading(true);
    try {
      await apiClient.put(`/api/cart/items/${item.id}`, { quantity: newQty });
      setQuantity(newQty);
      router.refresh();
    } catch {
      // silent — quantity stays the same
    } finally {
      setLoading(false);
    }
  }

  async function removeItem() {
    setLoading(true);
    try {
      await apiClient.delete(`/api/cart/items/${item.id}`);
      router.refresh();
    } catch {
      setLoading(false);
    }
  }

  return (
    <tr className="border-b border-border last:border-0">
      <td className="px-4 py-3 font-medium">{item.productName}</td>
      <td className="px-4 py-3 text-right">฿{item.unitPrice.toLocaleString()}</td>
      <td className="px-4 py-3">
        <div className="flex items-center justify-center gap-2">
          <button
            onClick={() => updateQuantity(quantity - 1)}
            disabled={loading || quantity <= 1}
            className="h-6 w-6 rounded border border-border flex items-center justify-center text-sm disabled:opacity-50"
            aria-label="ลดจำนวน"
          >
            −
          </button>
          <span className="w-8 text-center">{quantity}</span>
          <button
            onClick={() => updateQuantity(quantity + 1)}
            disabled={loading}
            className="h-6 w-6 rounded border border-border flex items-center justify-center text-sm disabled:opacity-50"
            aria-label="เพิ่มจำนวน"
          >
            +
          </button>
        </div>
      </td>
      <td className="px-4 py-3 text-right">
        ฿{item.subtotal.toLocaleString()}
      </td>
      <td className="px-4 py-3 text-right">
        <button
          onClick={removeItem}
          disabled={loading}
          className="text-xs text-red-500 hover:text-red-700 disabled:opacity-50"
          aria-label={`ลบ ${item.productName}`}
        >
          ลบ
        </button>
      </td>
    </tr>
  );
}
