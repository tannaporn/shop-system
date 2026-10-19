export type CartItem = {
  id: string;
  productId: string;
  productName: string;
  unitPrice: number;
  subtotal: number;
  quantity: number;
};

export type Cart = {
  cartId: string;
  items: CartItem[];
  total: number;
};
