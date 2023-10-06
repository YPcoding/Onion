<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode=='show'" ref="dialogForm" label-width="100px" label-position="left">
			<el-form-item label="角色名称" prop="roleName">
				<el-input v-model="form.roleName" clearable></el-input>
			</el-form-item>
			<el-form-item label="角色别名" prop="roleCode">
				<el-input v-model="form.roleCode" clearable></el-input>
			</el-form-item>
			<el-form-item label="排序" prop="sort">
				<el-input-number v-model="form.sort" controls-position="right" :min="0" style="width: 100%;"></el-input-number>
			</el-form-item>
			<el-form-item label="是否有效" prop="isActive">
				<el-switch v-model="form.isActive" :active-value="true" :inactive-value="false"></el-switch>
			</el-form-item>
			<el-form-item label="描述" prop="description">
				<el-input v-model="form.description" clearable type="textarea"></el-input>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button v-if="mode!='show'" type="primary" :loading="isSaveing" @click="submit()">保 存</el-button>
		</template>
	</el-dialog>
</template>

<script>
	export default {
		emits: ['success', 'closed'],
		data() {
			return {
				mode: "add",
				titleMap: {
					add: '新增',
					edit: '编辑',
					show: '查看'
				},
				visible: false,
				isSaveing: false,
				//表单数据
				form: {
					roleId:"",
					roleName: "",
					roleCode: "",
					sort: 0,
					isActive: true,
					description: "",
					concurrencyStamp:""
				},
				//验证规则
				rules: {
					sort: [
						{required: true, message: '请输入排序', trigger: 'change'}
					],
					roleName: [
						{required: true, message: '请输入角色名称'}
					],
					roleCode: [
						{required: true, message: '请输入角色别名'}
					],
					description: [
						{required: true, message: '请输入描述'}
					]
				}
			}
		},
		mounted() {

		},
		methods: {
			//显示
			open(mode='add'){
				this.mode = mode;
				this.visible = true;
				return this
			},
			//表单提交方法
			submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						let res = null
						if (this.mode === 'add') {
							res =  await this.$API.system.role.add.post(this.form)
						} else {
							res =  await this.$API.system.role.update.put(this.form)
						}
						this.isSaveing = false;
						if(res.succeeded){					
							this.$emit('success', this.form, this.mode)
							this.visible = false;
							this.$message.success("操作成功")
						}else{
							this.$alert(res.error, "提示", {type: 'error'})
						}
					}
				})
			},
			//表单注入数据
			setData(data){
				this.form.roleId = data.roleId
				this.form.roleName = data.roleName
				this.form.roleCode = data.roleCode
				this.form.sort = data.sort
				this.form.isActive = data.isActive
				this.form.description = data.description
				this.form.concurrencyStamp = data.concurrencyStamp

				//可以和上面一样单个注入，也可以像下面一样直接合并进去
				//Object.assign(this.form, data)
			}
		}
	}
</script>

<style>
</style>
