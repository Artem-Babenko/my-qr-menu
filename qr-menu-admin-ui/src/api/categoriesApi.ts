import { apiClient } from './api';
import type {
  CategoryView,
  CreateCategoryRequest,
  UpdateCategoryRequest,
} from '@/types/categories';

export const categoriesApi = {
  byNetwork: (networkId: number) =>
    apiClient.get<CategoryView[]>(`categories/by-network/${networkId}`),

  create: (payload: CreateCategoryRequest) =>
    apiClient.post<CategoryView>('categories/create', payload),

  update: (categoryId: number, payload: UpdateCategoryRequest) =>
    apiClient.put<CategoryView>(`categories/${categoryId}`, payload),

  delete: (categoryId: number) =>
    apiClient.delete<void>(`categories/${categoryId}`),
};

