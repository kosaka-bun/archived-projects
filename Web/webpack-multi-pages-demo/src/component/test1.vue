<template>
  <div>
    <div>
      <span style="margin-right: 1em;">this is a common vue component test1.</span>
      <el-date-picker v-model="date" type="date" placeholder="选择日期" />
      <div>BASE_URL: {{ baseUrl }}</div>
    </div>
  </div>
</template>

<script>
export default {
  name: "test1",
  data() {
    return {
      date: null,
      baseUrl: process.env.BASE_URL
    }
  }
}
</script>

<style scoped>

</style>