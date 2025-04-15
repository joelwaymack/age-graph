interface Connection {
	fromVertex: Vertex;
	edge: Edge;
	toVertex: Vertex;
}

interface Vertex {
	label: string;
	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	properties: any;
}

interface Edge {
	type: string;
	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	properties: any;
}

export type { Connection, Vertex, Edge };
