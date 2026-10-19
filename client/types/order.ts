export type OrderStatus = "Pending" | "Confirmed" | "Completed" | "Cancelled";

export type OrderItem = {
  id: string;
  productId: string;
  productName: string;
  price: number;
  quantity: number;
};

// shortcut: Order.totalPrice matches backend's totalPrice; items.productName and items.price may not match — verify at runtime
export type Order = {
  id: string;
  status: OrderStatus;
  totalPrice: number;
  items: OrderItem[];
  customerName?: string;
  createdAt: string;
  updatedAt: string;
};
