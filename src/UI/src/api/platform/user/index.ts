import request from '@/config/axios'
import type { UserAddType } from './types'


export const userAddApi = (data: UserAddType): Promise<IResponse<UserAddType>> => {
  return request.post({ url: '/api/User/Add', data })
}
