export interface ProductPriceItem {
  establishmentId: number;
  price: number;
  isActive: boolean;
}

export interface ProductView {
  id: number;
  name: string;
  description: string;
  networkId: number;
  categoryId: number;
  prices: ProductPriceItem[];
}

export interface CreateProductRequest {
  name: string;
  description: string;
  networkId: number;
  categoryId: number;
  prices: ProductPriceItem[];
}

export interface UpdateProductRequest {
  name: string;
  description: string;
  categoryId: number;
  prices: ProductPriceItem[];
}

