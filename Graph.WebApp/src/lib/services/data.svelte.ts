import type { Connection } from '$lib/types/connection';

const apiUrl = 'http://localhost:5054/';

export const getDataService = () => {
	return {
		getAllConnections: () => {
			return fetch(`${apiUrl}connections`, {
				method: 'GET',
				headers: {
					'Content-Type': 'application/json'
				}
			}).then(async (response): Promise<Connection[]> => await response.json());
		}
	};
};
