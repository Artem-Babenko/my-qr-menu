import { apiClient } from './api';
import type {
  CreateTableRequest,
  TableView,
  UpdateTableRequest,
} from '@/types/tables';

export const tablesApi = {
  byEstablishment: (establishmentId: number) =>
    apiClient.get<TableView[]>(`tables/by-establishment/${establishmentId}`),

  create: (payload: CreateTableRequest) =>
    apiClient.post<TableView>('tables/create', payload),

  update: (tableId: number, payload: UpdateTableRequest) =>
    apiClient.put<TableView>(`tables/${tableId}`, payload),

  delete: (tableId: number) => apiClient.delete<void>(`tables/${tableId}`),
};
