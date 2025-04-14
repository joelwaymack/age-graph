interface Connection {
	fromVertex: GraphItem;
	edge: GraphItem;
	toVertex: GraphItem;
}

interface GraphItem {
	label: string;
	// eslint-disable-next-line @typescript-eslint/no-explicit-any
	properties: any;
}

export type { Connection, GraphItem };
