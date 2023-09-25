import { http } from "@/utils/http";

type Result = {
  succeeded: boolean;
  data?: object;
  errors: Array<string>;
  errorMessage: string;
  code: number;
};

type ResultArray = {
  succeeded: boolean;
  data?: Array<any>;
  errors: Array<string>;
  errorMessage: string;
  code: number;
};

type ResultTable = {
  succeeded: boolean;
  errors: Array<any>;
  errorMessage: string;
  code: number;
  data?: {
    /** 总条目数 */
    totalItems?: number;
    /** 当前页数 */
    currentPage?: number;
    /** 每页大小 */
    pageSize?: number;
    /** 总页数 */
    totalPages?: number;
    /** 有上一页 */
    hasPreviousPage?: boolean;
    /** 有下一页 */
    hasNextPage?: boolean;
    /** 列表数据 */
    items: Array<any>;
  };
};

/** 获取用户管理列表 */
export const getRoleList = (data?: object) => {
  return http.request<ResultTable>("post", "/api/Role/PaginationQuery", {
    data
  });
};

/** 获取角色权限 */
export const getRolePermissionsByRoleId = (roleId: number) => {
  return http.request<ResultArray>(
    "get",
    `/api/Role/Query/Permission/By/${roleId}`
  );
};

/** 新增角色 */
export const addRole = (data?: object) => {
  return http.request<Result>("post", "/api/Role/Add", {
    data
  });
};

/** 修改角色 */
export const updateRole = (data?: object) => {
  return http.request<Result>("put", "/api/Role/Update", {
    data
  });
};

/** 修改角色权限菜单 */
export const updateRolePermissionMenu = (data?: object) => {
  return http.request<Result>("put", "/api/Role/Permission/Menu", {
    data
  });
};

/** 批量删除角色 */
export const onbatchDeleteRole = (data?: object) => {
  return http.request<Result>("delete", "/api/Role/Delete", {
    data
  });
};
