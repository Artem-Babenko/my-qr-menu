export interface MenuCategory {
  id: number;
  name: string;
  description: string | null;
  sortOrder: number;
  products: MenuProduct[];
}

export interface MenuProduct {
  id: number;
  name: string;
  description: string;
  price: number;
}
