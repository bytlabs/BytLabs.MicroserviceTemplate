import { ApolloClient, InMemoryCache, HttpLink, ApolloLink } from '@apollo/client';
import { setContext } from '@apollo/client/link/context';
import { getToken } from './config';

// Builds an ApolloClient pointed at the API's GraphQL endpoint (from /console/config), attaching the
// console's bearer token when present.
export function makeApolloClient(graphqlEndpoint: string) {
  const httpLink = new HttpLink({ uri: graphqlEndpoint });
  const authLink = setContext((_, { headers }) => {
    const token = getToken();
    return {
      headers: {
        ...headers,
        ...(token ? { Authorization: `Bearer ${token}` } : {}),
        Tenant: 'default',
      },
    };
  });
  return new ApolloClient({ link: ApolloLink.from([authLink, httpLink]), cache: new InMemoryCache() });
}
