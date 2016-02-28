import Ember from 'ember';

export default Ember.Controller.extend({
    queryParams: ['q', 'skip', 'take'],
    q: null,
    skip: 0,
    take: 10
    // actions: {
    //     search: function() {
    //         var searchText =this.get('searchText');
    //         alert(searchText);
    //          
    //         this.set('q', searchText);
    //     }
    // }    
});