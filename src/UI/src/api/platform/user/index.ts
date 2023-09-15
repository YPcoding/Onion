import request from '@/config/axios'
import type { UserAddType, UserListType, UserQueryType } from './types'

export const userAddApi = (data: UserAddType): Promise<IResponse<UserAddType>> => {
  return request.post({ url: '/api/User/Add', data })
}

export const getUserListApi = (data: UserQueryType): Promise<IResponse<UserListType>> => {
  return request.post({ url: '/api/User/PaginationQuery', data })
}
