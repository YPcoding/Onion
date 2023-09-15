<script setup lang="tsx">
import { Form, FormSchema } from '../../../../components/Form'
import { useForm } from '../../../../hooks/web/useForm'
import { PropType, reactive, watch, ref, unref, nextTick } from 'vue'
import { useValidator } from '../../../../hooks/web/useValidator'
import { useI18n } from '../../../../hooks/web/useI18n'
import { ElTree, ElCheckboxGroup, ElCheckbox } from 'element-plus'
import { userAddApi } from '../../../../api/platform/user'
import { UserAddType } from '../../../../api/platform/user/types'
import { filter, eachTree } from '../../../../utils/tree'
import { findIndex } from '../../../../utils'

const { t } = useI18n()

const { required } = useValidator()

const props = defineProps({
  currentRow: {
    type: Object as PropType<any>,
    default: () => null
  }
})

const treeRef = ref<typeof ElTree>()

const formSchema = ref<FormSchema[]>([
  {
    field: 'userName',
    label: '用户名',
    component: 'Input'
  },
  {
    field: 'email',
    label: '邮箱',
    component: 'Input'
  },
  {
    field: 'phoneNumber',
    label: '手机号码',
    component: 'Input'
  },
  {
    field: 'password',
    label: '密码',
    component: 'Input'
  }
])

const currentTreeData = ref()
const nodeClick = (treeData: any) => {
  currentTreeData.value = treeData
}

const rules = reactive({
  roleName: [required()],
  role: [required()],
  status: [required()]
})

const { formRegister, formMethods } = useForm()
const { setValues, getFormData, getElFormExpose } = formMethods

const treeData = ref([])

const submit = async () => {
  const elForm = await getElFormExpose()
  const valid = await elForm?.validate().catch((err) => {
    console.log(err)
  })
  if (valid) {
    const formData = await getFormData<UserAddType>()
    const res = await userAddApi(formData)
    if (res.code === 0) {
      console.log(res)
    }
    return formData
  }
}

watch(
  () => props.currentRow,
  (currentRow) => {
    if (!currentRow) return
    setValues(currentRow)
  },
  {
    deep: true,
    immediate: true
  }
)

defineExpose({
  submit
})
</script>

<template>
  <Form :rules="rules" @register="formRegister" :schema="formSchema" />
</template>
