<template>
	<el-dialog :title="titleMap[mode]" v-model="visible" :width="500" destroy-on-close @closed="$emit('closed')">
		<el-form :model="form" :rules="rules" :disabled="mode=='show'" ref="dialogForm" label-width="100px">
			<el-form-item label="上级部门" prop="superiorId">
				<el-cascader v-model="form.superiorId"  :options="groups" :props="groupsProps" :show-all-levels="true" clearable style="width: 100%;"></el-cascader>
			</el-form-item>
			<el-form-item label="部门名称" prop="departmentName">
				<el-input v-model="form.departmentName" placeholder="请输入部门名称" clearable></el-input>
			</el-form-item>
			<el-form-item label="排序" prop="sort">
				<el-input-number v-model="form.sort" controls-position="right" :min="1" style="width: 100%;"></el-input-number>
			</el-form-item>
			<el-form-item label="是否有效" prop="isActive">
				<el-switch v-model="form.isActive" :active-value="true" :inactive-value="false"></el-switch>
			</el-form-item>
			<el-form-item label="备注" prop="description">
				<el-input v-model="form.description" clearable type="textarea"></el-input>
			</el-form-item>
		</el-form>
		<template #footer>
			<el-button @click="visible=false" >取 消</el-button>
			<el-button v-if="mode!='show'" type="primary" :loading="isSaveing" @click="submit()" v-auths="['department.add','department.update']">保 存</el-button>
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
					parentId:null,
					departmentId:null,
					superiorId: null,
					departmentName: "",
					sort: 1,
					isActive: true,
					description: "",
					concurrencyStamp:""
				},
				//验证规则
				rules: {
					sort: [
						{required: true, message: '请输入排序', trigger: 'change'}
					],
					departmentName: [
						{required: true, message: '请输入部门名称'}
					],
					isActive: [
						{required: true, message: '请输入选择是否激活'}
					],
					description: [
						{required: true, message: '请输入备注'}
					]
				},
				//所需数据选项
				groups: [],
				groupsProps: {
					value: "id",
					emitPath: false,
					checkStrictly: true,
					children:"children"
				}
			}
		},
		mounted() {
			this.getGroup()
		},
		methods: {
			//显示
			open(mode='add'){
				this.mode = mode;
				this.visible = true;
				return this
			},
			//加载树数据
			async getGroup(){
				 var res = await this.$API.system.dept.list.post();
				 this.groups = this.convertToElTreeData(res.data);
			},
			//表单提交方法
			submit(){
				this.$refs.dialogForm.validate(async (valid) => {
					if (valid) {
						this.isSaveing = true;
						let res = null
						if (this.mode === 'add') {
							 res = await this.$API.system.dept.add.post(this.form);
						} else {
							res =  await this.$API.system.dept.update.put(this.form)
						}
						if(res.succeeded){
							this.$emit('success', this.form, this.mode)
							this.visible = false;
							this.$message.success("操作成功")
						}else{
							this.$alert(res.error, "提示", {type: 'error'})
						}					
						this.isSaveing = false;
					}
				})
			},
			//表单注入数据
			setData(data){
				this.form.departmentId = data.id
				this.form.departmentName = data.departmentName
				this.form.isActive = data.isActive
				this.form.sort = data.sort
				this.form.superiorId = data.parentId
				this.form.parentId =data.parentId
				this.form.description = data.description
				this.form.concurrencyStamp = data.concurrencyStamp

				//可以和上面一样单个注入，也可以像下面一样直接合并进去
				//Object.assign(this.form, data)
			},
			convertToElTreeData(data, parentId = null) {
                const treeData = [];
                for (const item of data) {
                    if ((item.superiorId === parentId) || (parentId === null && !item.superiorId)) {
                        const children = this.convertToElTreeData(data, item.id);
                        const treeNode = {
                            id: item.id,
                            label: item.departmentName,
                            children: children.length > 0 ? children : null,
                        };
                        treeData.push(treeNode);
                    }
                }
                return treeData;
            },
		}
	}
</script>

<style>
</style>
