import Ember from 'ember';

export default Ember.Route.extend({
    model: function() {
        return this.store.query('post', { 
            skip: 0, 
            take: 10 
        });
    } 
});