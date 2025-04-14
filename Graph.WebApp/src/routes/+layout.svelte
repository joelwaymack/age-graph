<script lang="ts">
	import '../app.css';
	let { children } = $props();
</script>

<div class="h-full p-4">
	{@render children()}
</div>
