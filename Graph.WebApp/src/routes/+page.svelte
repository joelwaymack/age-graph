<script lang="ts">
	import { getDataService } from '$lib/services/data.svelte';
	import { onMount } from 'svelte';
	import type { Connection } from '$lib/types/connection';
	import type { LinkObject, NodeObject } from 'force-graph';

	const dataService = getDataService();

	let connections = $state<Connection[]>([]);
	let nodes = $derived.by<NodeObject[]>(() => {
		const nodes: NodeObject[] = [];

		connections.forEach((c) => {
			const fromVertexId = JSON.stringify(c.fromVertex);
			if (!nodes.find((n) => n.id === fromVertexId)) {
				nodes.push({
					id: fromVertexId
				});
			}

			const toVertexId = JSON.stringify(c.toVertex);
			if (!nodes.find((n) => n.id === toVertexId)) {
				nodes.push({
					id: toVertexId
				});
			}
		});

		return nodes;
	});
	let links = $derived<LinkObject[]>(
		connections.map((c) => ({
			source: JSON.stringify(c.fromVertex),
			target: JSON.stringify(c.toVertex),
			edge: JSON.stringify(c.edge),
			color: c.edge.type === 'Contains' ? 'white' : 'red'
		}))
	);

	let graph = $state();

	onMount(async () => {
		const ForceGraph = await import('force-graph');
		connections = await dataService.getAllConnections();

		const graphElement = document.getElementById('graph');

		if (graphElement) {
			const graphData = {
				nodes: nodes,
				links: links
			};
			graph = new ForceGraph.default(graphElement)
				.linkDirectionalParticles(2)
				.graphData(graphData)
				.nodeId('id')
				.nodeLabel('id')
				.linkLabel('edge')
				.linkColor('color')
				.height(graphElement.clientHeight - 70)
				.width(graphElement.clientWidth);
		}
	});
</script>

<div class="flex h-full flex-col">
	<h1 class="text-2xl font-bold">Graph</h1>
	<div class="grow">
		<div id="graph" class="h-full rounded border"></div>
	</div>
</div>
