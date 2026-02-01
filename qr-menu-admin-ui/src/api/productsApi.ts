import { apiClient } from './api';
import type {
  CreateProductRequest,
  ProductView,
  UpdateProductRequest,
} from '@/types/products';

export const productsApi = {
  byNetwork: (networkId: number) =>
    apiClient.get<ProductView[]>(`products/by-network/${networkId}`),

  create: (payload: CreateProductRequest) =>
    apiClient.post<ProductView>('products/create', payload),

  update: (productId: number, payload: UpdateProductRequest) =>
    apiClient.put<ProductView>(`products/${productId}`, payload),

  delete: (productId: number) => apiClient.delete<void>(`products/${productId}`),
};

