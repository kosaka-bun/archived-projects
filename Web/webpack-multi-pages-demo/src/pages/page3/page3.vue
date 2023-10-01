<template>
  <div>
    <h1>wepack test page3</h1>
    <test1 />
    <div class="scoped-style">{{ text }}</div>
    <a href="./page1.html">back</a>
  </div>
</template>

<script>
import Test1 from '@/component/test1.vue'

export default {
  name: "page3",
  components: { Test1 },
  data() {
    return {
      text: 'this is scoped style.'
    }
  }
}
</script>

<style scoped>
.scoped-style {
  color: green;
}
</style>