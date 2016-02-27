import DS from 'ember-data';

export default DS.Model.extend({
    text: DS.attr('string'),
    creationDate: DS.attr('date'),
    user: DS.belongsTo('user', { async: true }),
    post: DS.belongsTo('post', { async: true })
});
