import Ember from 'ember';

export default Ember.Route.extend({
    queryParams: {
        q: {
            refreshModel: true,
            replace: true          
        },
        skip: {
            refreshModel: true,
            replace: true          
        },    
        take: {
            refreshModel: true,
            replace: true          
        }   
    },
	model: function(params) {
        return this.store.query('post', {
            q: params.q,
            skip: params.skip, 
            take: params.take 
        });
    }
});