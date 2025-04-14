<script lang="ts">
	import { curveBasis } from 'd3-shape';
	import { cubicOut } from 'svelte/easing';

	import { Chart, Dagre, Group, nodesFromLinks, Rect, Spline, Svg, Text } from 'layerchart';
	import { getDataService } from '$lib/services/data.svelte';
	import { onMount } from 'svelte';

	const dataService = getDataService();

	let settings: any = {
		ranker: 'network-simplex',
		direction: 'left-right',
		align: 'up-left',
		nodeSeparation: 50,
		rankSeparation: 50,
		edgeSeparation: 10,
		edgeLabelPosition: 'center',
		edgeLabelOffset: 10,
		curve: curveBasis,
		arrow: 'arrow'
	};

	let data = $state({
		nodes: [],
		edges: []
	});

	onMount(async () => {
		const newData = await dataService.getAllConnections();
		const revisedData: {nodes: any[], edges: any[]} = {
			nodes: [],
			edges: []
		};
		newData.forEach(connection => {
			console.log('connection', connection);
			const from = `${connection.fromVertex.label}-${connection.fromVertex.properties.id} ${connection.fromVertex.properties.type}`;
			const to = `${connection.toVertex.label}-${connection.toVertex.properties.id} ${connection.toVertex.properties.type}`;
			revisedData.edges.push({
				source: from,
				target: to
			});
			revisedData.nodes.push({id: from});
			revisedData.nodes.push({id: to});
		});
		console.log('revisedData', revisedData);
		data = revisedData;
	});
</script>

<div class="flex gap-2">
	<div class="h-[500px] flex-1 overflow-hidden rounded border p-4">
		<Chart
			{data}
			transform={{
				mode: 'canvas',
				initialScrollMode: 'scale',
				tweened: { duration: 800, easing: cubicOut }
			}}
		>
			<Svg>
				<Dagre {data} edges={(d) => d.edges} {...settings} let:nodes let:edges>
					<g class="edges">
						{#each edges as edge, i (edge.v + '-' + edge.w)}
							<Spline
								data={edge.points}
								x="x"
								y="y"
								class="stroke-surface-content opacity-30"
								tweened
								curve={settings.curve}
								markerEnd={{ type: settings.arrow, class: 'stroke-2 stroke-white' }}
								stroke="white"
								strokeWidth={2}
							/>
						{/each}
					</g>

					<g class="nodes">
						{#each nodes as node (node.label)}
							<Group x={node.x - node.width / 2} y={node.y - node.height / 2} tweened>
								<Rect
									width={node.width}
									height={node.height}
									class="fill-surface-200 stroke-primary/50 stroke-2"
									rx={10}
								/>

								<Text
									value={node.label}
									x={node.width / 2}
									y={node.height / 2}
									dy={-2}
									textAnchor="middle"
									verticalAnchor="middle"
									class="pointer-events-none text-xs"
								/>
							</Group>
						{/each}
					</g>
				</Dagre>
			</Svg>
		</Chart>
	</div>
</div>
