import { apiClient } from './api';
import type {
  CreateProductRequest,
  ProductView,
  UpdateProductRequest,
} from '@/types/products';

export const productsApi = {
  byNetwork: (networkId: number) =>
    apiClient.get<ProductView[]>(`products/by-network/${networkId}`),

  lookup: (establishmentId: number, q: string) =>
    apiClient.get<{ id: number; name: string; categoryName: string; price: number }[]>(
      `products/lookup`,
      { params: { establishmentId, q } },
    ),

  create: (payload: CreateProductRequest) =>
    apiClient.post<ProductView>('products/create', payload),

  update: (productId: number, payload: UpdateProductRequest) =>
    apiClient.put<ProductView>(`products/${productId}`, payload),

  delete: (productId: number) => apiClient.delete<void>(`products/${productId}`),
};

