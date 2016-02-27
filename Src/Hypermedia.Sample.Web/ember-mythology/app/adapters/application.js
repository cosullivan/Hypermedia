import Ember from 'ember';
import DS from 'ember-data';
import ENV from "../config/environment";

var ApplicationAdaptor = DS.JSONAPIAdapter.extend({
	host: ENV.APP.APIEndpoint,
	namespace: 'v1',
});

export default ApplicationAdaptor;