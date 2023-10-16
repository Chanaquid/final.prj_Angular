export interface Pagination<T> {
  index: number;
  pageSize: number;
  count: number;
  data: T;
}
